#pragma once

#define EXPORT __declspec(dllexport)
#define OUT // Just to denote OUT parameters

#include <string>

namespace DirkaLurk { namespace Library
{
    const char* const AUTHOR = "Dirk";
    const char* const PURPOSE = "To voyage from C to C";

    class EXPORT Client final
    {
    public:
        explicit Client(std::string id);
        ~Client() = default;

        void DoTheThing(void (__stdcall* function)(const char*), const char* message) const;

        std::uint8_t  GenerateRandom8Bit() const;
        std::uint16_t GenerateRandom16Bit() const;
        std::uint32_t GenerateRandom32Bit() const;
        std::uint64_t GenerateRandom64Bit() const;

        char GenerateRandomChar() const;
        std::string GenerateRandomUUID() const;

        std::string GetId() const;

    private:
        class Impl;
        std::unique_ptr<Impl> impl;
        Impl* GetImpl() const { return impl.get(); };
    };
}}

extern "C" {
    EXPORT std::uint8_t GenerateRandom8Bit();
    EXPORT std::uint16_t GenerateRandom16Bit();
    EXPORT std::uint32_t GenerateRandom32Bit();
    EXPORT std::uint64_t GenerateRandom64Bit();

    EXPORT void GenerateRandomUUID(OUT char* uuid);
    EXPORT char GenerateRandomChar();
    EXPORT void GenerateRandomString(OUT char* str, const int length);

    EXPORT char ToUpperChar(char c);
    EXPORT void ToUpperString(OUT char* lower, const char* input, const int length);

    EXPORT void InvokeMe(void (__stdcall* function)(const char*), const char* message, int length);
    EXPORT void DelayInvoke(void(__stdcall* function)(), int delayMillis);
    EXPORT void PollInvoke();
}
